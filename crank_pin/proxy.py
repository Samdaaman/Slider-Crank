import serial
import time
import socket
from typing import Optional

LINE1 = b'Crank init begin\n'
LINE2 = b'Conrod Received\n'
LINE3 = b'ACK\n'


def handshake() -> Optional[serial.Serial]:
    try:
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
                return ser
    except Exception as e:
        print(str(e))


def begin_proxy(ser: serial.Serial):
    time.sleep(5)
    sock = socket.socket(family=, type=, proto=)
    sock.

def init():
    ser = handshake()
    if ser is None:
        print('Failed to complete initial handshake')
        exit(1)
    else:
        begin_proxy(ser)


if __name__ == "__main__":
    init()
