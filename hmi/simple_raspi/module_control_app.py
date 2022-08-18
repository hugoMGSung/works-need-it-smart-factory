# QT Designer 연동 소스
from PyQt5 import QtGui, QtWidgets, uic
from PyQt5.QtWidgets import *
from PyQt5.QtCore import *
from PyQt5.QtGui import *
import sys
# 여기에 GPIO 작업 및 MQTT 송수신을 처리함

class MyWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        uic.loadUi('./iot_design_window.ui', self)

        #ui에 있는 위젯하고 시그널 처리(컨트롤 이벤트처리)
        self.btnOn.clicked.connect(self.btnOn_Clicked)
        self.btnOff.clicked.connect(self.btnOff_Clicked)
        self.lblResult.setText('')

    def btnOn_Clicked(self):
        print('LED를 켰습니다!!')
        self.lblResult.setText('LED를 켰습니다')

    def btnOff_Clicked(self):
        print('LED를 껐습니다.')
        self.lblResult.setText('LED를 껐습니다')

if __name__ == '__main__':
    app = QApplication(sys.argv)
    win = MyWindow()
    win.show()
    sys.exit(app.exec_())