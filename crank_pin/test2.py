import serial
import time

ser = serial.Serial('COM6', 9600, timeout=3)
time.sleep(2)
ser.write(b'abc\n')
print(ser.readline())
print(ser.read())
