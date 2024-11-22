import socket
from collections import deque
import numpy as np
from scipy.stats import linregress
from minisom import MiniSom

# ソケットサーバー設定
HOST = '127.0.0.1'
PORT = 6000

# プレイヤータイプのラベル
label_mapping = {0: 'Killer', 1: 'Achiever', 2: 'Explorer'}


class PlayerClassifier:
    def __init__(self, smoothing_factor=0.3, log_window=10):
        self.smoothing_factor = smoothing_factor
        self.log_window = log_window
        self.logs = deque(maxlen=log_window)  # 過去のログを保持（最大log_window件）
        self.currentScores = {}

    def add_log(self, new_log):
        """
        新しいログを追加し、古いログを削除。
        """
        self.logs.append(new_log)

    def preprocess_features(self, alpha=0.3):
        """
        特徴量を前処理する。

        Args:
        - alpha (float): エクスポーネンシャルフィルターの平滑化係数（0 < alpha <= 1）

        短期的な時系列を考慮して、スコアの統合時に過去のスコアと統合することで長期的なスコアを考慮する方針。
        Returns:
        - features: Dict 各タイプの特徴量
        """
        if not self.logs:
            return None

        # ログデータにエクスポーネンシャルフィルターを適用
        def apply_exponential_filter(scores):
            filtered_scores = []
            ema = 0  # 初期値（0でも問題ない）
            for score in scores:
                ema = alpha * score + (1 - alpha) * ema  # EMA計算
                filtered_scores.append(ema)
            return filtered_scores

        killer_scores, achiever_scores, explorer_scores = [], [], []

        for log in self.logs:
            killer_scores.append(log.get("Killer", 0))
            achiever_scores.append(log.get("Achiever", 0))
            explorer_scores.append(log.get("Explorer", 0))

        # Exponentialフィルター適用
        killer_scores = apply_exponential_filter(killer_scores)
        achiever_scores = apply_exponential_filter(achiever_scores)
        explorer_scores = apply_exponential_filter(explorer_scores)

        # 傾向の特徴量を計算
        def calculate_trend_features(scores):
            slope = linregress(range(len(scores)), scores).slope if len(scores) > 1 else 0
            variance = np.var(scores)  # 安定度
            total = sum(scores)  # 合計値
            return slope, variance, total

        return {
            "Killer": calculate_trend_features(killer_scores),
            "Achiever": calculate_trend_features(achiever_scores),
            "Explorer": calculate_trend_features(explorer_scores),
        }

    def update_score_with_history(self, scores, new_weight=0.7, current_weight=0.3):
        """
        過去のスコアを考慮してスコアを更新。
        Args:
            scores (dict): 新しいスコア（辞書形式で、各タイプがキー、スコアが値）
            new_weight (float): 新しいスコアの重み
            current_weight (float): 現在のスコアの重み
        Returns:
            dict: 更新されたスコア（辞書形式）
        """
        # 初回呼び出し時は currentScores を初期化
        if not self.currentScores:
            self.currentScores = scores
            return scores  # 初回はそのまま返す
    
        # スコア辞書のキーが一致するかチェック
        if set(scores.keys()) != set(self.currentScores.keys()):
            raise ValueError("入力されたスコア辞書のキーが現在のスコア辞書と一致しません。")
    
        # スコアを更新
        updated_scores = {}
        for key in scores:
            updated_scores[key] = scores[key] * new_weight + self.currentScores[key] * current_weight

        """
        # デバッグ表示
        print("過去のスコア: ", {key: round(value, 2) for key, value in self.currentScores.items()})
        print("過去を考慮した現在のスコア: ", {key: round(value, 2) for key, value in scores.items()})
        """
        
        # 更新後のスコアを反映
        self.currentScores = updated_scores
        return updated_scores
        


def classify_base_type(features):
    slope_scores = {key: slope for key, (slope, _, _) in features.items()}

    # 通常の分類処理
    most_increasing = max(slope_scores, key=slope_scores.get)
    most_decreasing = min(slope_scores, key=slope_scores.get)
    remaining_key = (set(slope_scores.keys()) - {most_increasing, most_decreasing}).pop()

    # 信頼度を計算（仮にスコアの正規化値を使用）
    max_slope = max(slope_scores.values()) if slope_scores else 1  # もしスロープがゼロしかない場合は1に設定
    normalized_scores = {key: slope / max_slope if max_slope != 0 else 0 for key, slope in slope_scores.items()}

    return [most_increasing, remaining_key, most_decreasing], normalized_scores


# SOMモデルの初期化
som = MiniSom(x=1, y=3, input_len=9, sigma=0.5, learning_rate=0.5)
som.random_weights_init(np.random.rand(3, 9))


def integrate_predictions(base_type_result, som_result, base_weight=0.7, som_weight=0.3):
    """
    ルールベースとSOMの分類結果を加重平均で統合。
    """
    base_type, base_scores = base_type_result
    som_type, som_scores = som_result

    # 両スコアを加重平均
    combined_scores = {}
    for key in base_scores:
        combined_scores[key] = base_scores[key] * base_weight + som_scores[key] * som_weight

    # 過去のスコアの影響を反映
    combined_scores = classifier.update_score_with_history(combined_scores)

    # 最終的な分類はスコア最大値で決定
    final_type = max(combined_scores, key=combined_scores.get)
    return final_type, combined_scores


def parse_action_logs(log_data):
    """
    セミコロン区切りのログデータを辞書形式に変換。
    """
    logs = []
    for log_entry in log_data.split(";"):
        if not log_entry.strip():
            continue
        try:
            log_dict = {}
            for pair in log_entry.split(","):
                key, value = pair.split(":")
                log_dict[key.strip()] = int(value.strip())
            logs.append(log_dict)
        except ValueError:
            print(f"Invalid log format: {log_entry}")
            return None
    return logs


def handle_client(conn, addr, classifier):
    with conn:
        print("Connected by", addr)
        while True:
            # データ受信
            data = conn.recv(1024)
            if not data:
                break

            # データのデコードと解析
            log_data = data.decode('utf-8').strip()
            new_logs = parse_action_logs(log_data)
            if not new_logs:
                conn.sendall("Invalid log format".encode('utf-8'))
                continue

            # ログを追加
            for log in new_logs:
                classifier.add_log(log)

            # 特徴量抽出
            features = classifier.preprocess_features()
            if not features:
                conn.sendall("Insufficient data".encode('utf-8'))
                continue

            # ルールベース分類
            base_type_result = classify_base_type(features)

            # SOM分類
            feature_vector = [
                features["Killer"][0], features["Achiever"][0], features["Explorer"][0],  # 傾き
                features["Killer"][1], features["Achiever"][1], features["Explorer"][1],  # 分散
                features["Killer"][2], features["Achiever"][2], features["Explorer"][2],  # 合計値
            ]
            winner = som.winner(feature_vector)
            som_type = label_mapping[winner[1]]
            som_scores = {label_mapping[idx]: 1.0 if idx == winner[1] else 0.0 for idx in range(3)}

            # 統合結果
            final_prediction, confidence_scores = integrate_predictions(base_type_result, (som_type, som_scores))
            # スコアを小数点第2位まで丸める
            confidence_scores = {key: round(value, 2) for key, value in confidence_scores.items()}

            # 結果を送信
            conn.sendall(str(final_prediction).encode('utf-8'))

            confirmation_log = {
                "type": final_prediction,
                "confidence_scores": confidence_scores
            }
            print(f"Prediction sent: {confirmation_log}")


# サーバー起動
classifier = PlayerClassifier(smoothing_factor=0.3, log_window=10)

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((HOST, PORT))
    s.listen()
    print("Server listening on", (HOST, PORT))
    while True:
        conn, addr = s.accept()
        handle_client(conn, addr, classifier)
