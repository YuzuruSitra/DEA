import socket
import numpy as np
from minisom import MiniSom
from scipy.stats import linregress  # 傾きを計算するために使用

# ソケットサーバー設定
HOST = '127.0.0.1'
PORT = 5000

# プレイヤータイプのラベル
label_mapping = {'Killer': 0, 'Achiever': 1, 'Explorer': 2}

# SOMモデルの初期化（特徴量の次元数に合わせてinput_lenを12に変更）
som = MiniSom(x=1, y=3, input_len=12, sigma=0.5, learning_rate=0.5)
som.random_weights_init(np.random.rand(1, 3, 12).reshape(3, 12))  # 初期化

def handle_client(conn, addr):
    with conn:
        print("Connected by", addr)
        while True:
            # データ受信
            data = conn.recv(1024)
            if not data:
                break

            # データのデコードと解析
            action_logs = data.decode('utf-8').strip().split(";")
            if not action_logs[-1]:
                action_logs = action_logs[:-1]  # 最後が空なら削除
            features = extract_features(action_logs)  # 特徴抽出

            if features is None:
                conn.sendall("Invalid data".encode('utf-8'))
                continue

            # ルールベースで初期分類
            base_type = classify_base_type(features)

            # SOMを用いたクラスタリングで分類結果の補強
            winner = som.winner(features)
            som.update(features, winner, 0.5, 0.5)
            cluster_type = winner[1]  # y座標をクラスタ番号とする

            # 結果を統合
            final_prediction = integrate_predictions(base_type, cluster_type)

            # 結果をクライアントに送信
            conn.sendall(str(final_prediction).encode('utf-8'))
            print(f"Prediction sent: {final_prediction}")

def extract_features(logs):
    # 各行動スコアとタイムスタンプのリストを生成
    killer_scores, achiever_scores, explorer_scores, time_stamps = [], [], [], []

    for log in logs:
        try:
            parts = log.split(", ")
            killer_score = int(parts[0].split("Killer: ")[1])
            achiever_score = int(parts[1].split("Achiever: ")[1])
            explorer_score = int(parts[2].split("Explorer: ")[1])
            time_elapsed = int(parts[3].split("Time: ")[1])

            killer_scores.append(killer_score)
            achiever_scores.append(achiever_score)
            explorer_scores.append(explorer_score)
            time_stamps.append(time_elapsed)
        except (IndexError, ValueError):
            print(f"Invalid log format: {log}")
            return None  # 無効なデータは無視

    if not (killer_scores and achiever_scores and explorer_scores and time_stamps):
        return None  # データが不足している場合

    # スコアの特徴量を計算
    total_killer = sum(killer_scores)
    total_achiever = sum(achiever_scores)
    total_explorer = sum(explorer_scores)

    slope_killer = linregress(range(len(killer_scores)), killer_scores).slope
    slope_achiever = linregress(range(len(achiever_scores)), achiever_scores).slope
    slope_explorer = linregress(range(len(explorer_scores)), explorer_scores).slope

    moving_avg_killer = np.mean(killer_scores[-3:]) if len(killer_scores) >= 3 else np.mean(killer_scores)
    moving_avg_achiever = np.mean(achiever_scores[-3:]) if len(achiever_scores) >= 3 else np.mean(achiever_scores)
    moving_avg_explorer = np.mean(explorer_scores[-3:]) if len(explorer_scores) >= 3 else np.mean(explorer_scores)

    variance_killer = np.var(killer_scores)
    variance_achiever = np.var(achiever_scores)
    variance_explorer = np.var(explorer_scores)

    return [
        total_killer, total_achiever, total_explorer,
        slope_killer, slope_achiever, slope_explorer,
        moving_avg_killer, moving_avg_achiever, moving_avg_explorer,
        variance_killer, variance_achiever, variance_explorer
    ]

def classify_base_type(features):
    # ルールベースでの分類：スコアの高いタイプに基づく基本分類
    total_killer, total_achiever, total_explorer = features[:3]
    threshold = 5  # スコア差の閾値

    killer_vs_others = total_killer - max(total_achiever, total_explorer)
    achiever_vs_others = total_achiever - max(total_killer, total_explorer)
    explorer_vs_others = total_explorer - max(total_killer, total_achiever)

    if killer_vs_others > threshold:
        return label_mapping['Killer']
    elif achiever_vs_others > threshold:
        return label_mapping['Achiever']
    elif explorer_vs_others > threshold:
        return label_mapping['Explorer']
    else:
        return None  # 中間状態として、SOMに任せる

def integrate_predictions(base_type, cluster_type):
    # ルールベースの予測とクラスタリングの結果を統合
    if base_type is not None:
        return base_type
    else:
        return cluster_type  # SOMのクラスター結果（0, 1, 2）

# サーバーの起動
with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((HOST, PORT))
    s.listen()
    print("Server listening on", (HOST, PORT))
    while True:
        conn, addr = s.accept()
        handle_client(conn, addr)
