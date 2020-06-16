from flask import Flask, request, Response
from flask_cors import CORS, cross_origin
import socket
import time

app = Flask(__name__)
cors = CORS(app)
app.config['CORS_HEADERS'] = 'Content-Type'

HOST = '127.0.0.1'
PORT = 726

sock = None


def initialise():
    global sock
    if sock is not None:
        sock.close()
    while True:
        try:
            sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            sock.connect((HOST, PORT))
            break
        except:
            print('Error connecting to socket, trying again....')
            time.sleep(3)


@app.route('/socket', methods=['POST', 'OPTIONS'])
@cross_origin()
def send_socket_data():
    global sock
    if request.method == 'OPTIONS':
        return Response(status=200)
    else:
        while True:
            try:
                data = bytes(request.data.decode('utf-8') + "\n", 'utf-8')
                sock.send(data)
                print(f'Successfully sent {data}')
                return Response(status=200)
            except Exception as e:
                initialise()


if __name__ == "__main__":
    app.run(port=727, debug=True)
