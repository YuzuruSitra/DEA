build command

cd - target sorcefile directory. 



command1 - pyinstaller --onefile --hidden-import numpy --hidden-import scipy --hidden-import scipy.stats --hidden-import minisom Main.py

command2 - pyinstaller --onefile --collect-all numpy --collect-all scipy --collect-all minisom Main.py