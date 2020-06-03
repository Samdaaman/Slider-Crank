import serial

import layer2
packets = [
    b'\x01abcdefgh\02',

    b'\x01123',
    b'456\02',
    
    b'\x01\x00\x00\x00\x01\x00\x02\x00a\x02',

    b'noise\x02\x01sam is cool\x02\x01cut off',
    b'... just kidding\x02'
]
for packet in packets:
    if layer2.update_data(packet):
        print(layer2.try_read())

# ser = serial.Serial('COM3')
# ser.write(b'yeet')
# print(ser.read_until())
