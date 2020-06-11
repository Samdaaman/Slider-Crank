import socket
import time
import json

HOST = '127.0.0.1'
PORT = 726

DATA_ON = f'{json.dumps({"power": 1})}\n'.encode('utf-8')
DATA_OFF = f'{json.dumps({"power": 0})}\n'.encode('utf-8')
DATA_EOF = b'<EOF>'


def initialise():
    while True:
        try:
            sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            sock.connect((HOST, PORT))
            return sock
        except:
            print('Error connecting to socket, trying again....')
            time.sleep(3)


def send_bytes(data: bytes, sock: socket.SocketType):
    sock.send(data)
    print(f'Successfully sent {data}')


def main():
        sock = initialise()
        while True:
            send_bytes(DATA_ON, sock)
            time.sleep(1)
            send_bytes(DATA_OFF, sock)
            time.sleep(1)
            # send_bytes(DATA_EOF, sock)
        sock.close()


if __name__ == "__main__":
    main()
