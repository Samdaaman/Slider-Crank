import socket
import time
import json

HOST = '127.0.0.1'
PORT = 726

DATA_ON = json.dumps({"power": True}).encode('utf-8')
DATA_OFF = json.dumps({"power": False}).encode('utf-8')

def Initialise():
    while True:
        try:
            sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            sock.connect((HOST, PORT))
            return sock
        except:
            print('Error connecting to socket, trying again....')
            time.sleep(3)


def sendBytes(data: bytes):
    sock = Initialise()
    sock.sendall(data)
    sock.close()
    print(f'Successfully sent {data}')


def main():
    while True:
        try:
            sendBytes(DATA_ON)
            time.sleep(10)
            sendBytes(DATA_OFF)
            time.sleep(10)
        except Exception as e:
            print(f'Exception occurred: {e}')
            time.sleep(5)


if __name__ == "__main__":
    main()
