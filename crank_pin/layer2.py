from typing import Optional

ESCAPE = b'\00'
START = b'\x01'
END = b'\x02'

_raw_stream = b''


def try_read(clear_buffer=True) -> Optional[bytes]:
    global _raw_stream
    if len(_raw_stream) > 0:
        if _raw_stream[0:1] == START:
            processed_stream = b''
            escape = False
            for i in range(1, len(_raw_stream)):
                byte = _raw_stream[i:i+1]
                if not escape:
                    if byte == START:
                        raise Exception('Why has try read reached another start bit')
                    elif byte == END:
                        if len(processed_stream) > 0:
                            if clear_buffer:
                                _raw_stream = _raw_stream[i+1:]
                            return processed_stream
                        else:
                            return None
                    elif byte == ESCAPE:
                        escape = True
                    else:
                        processed_stream += byte
                else:
                    processed_stream += byte
                    escape = False
        else:
            raise Exception('Raw stream should start with a start bit')


def update_data(input_data: bytearray) -> bool:
    global _raw_stream
    if hasattr(input_data, 'decode'):
        if len(input_data) > 0:
            if len(_raw_stream) == 0 and input_data[0:1] != START:
                if END + START in input_data:
                    input_data = input_data[input_data.index(END + START) + 1:]
            if len(_raw_stream) != 0 or input_data[0:1] == START:
                _raw_stream += input_data
            return try_read(False) is not None
    else:
        raise Exception('Input data must be a byte array')
