import serial
import time

LINE1 = b'Crank init begin\n'
LINE2 = b'Conrod Received\n'
LINE3 = b'ACK\n'


def handshake() -> bool:
    ser = serial.Serial('COM6', 9600, timeout=3)
    time.sleep(0.5)
    line1 = ser.readline()
    print(line1)
    if line1 == LINE1:
        ser.write(LINE2)
        print(LINE2)
        line3 = ser.readline()
        print(line3)
        if line3 == LINE3:
            time.sleep(5)
            return True


while not handshake():
    pass
