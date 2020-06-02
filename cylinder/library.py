import os
import csv
from typing import List, Tuple
import hashlib

PLAYLIST_FILE = 'playlist_map.csv'
LIBRARY_FILE = 'library_map.csv'
LIBRARY_FOLDER = 'library'


def get_filename(title, artist):
    return f'{title}_{artist}'.replace(' ', '-')


def download(title, artist, url) -> bool:
    os.system(f"youtube-dl.exe --audio-quality 9 --audio-format wav -o {LIBRARY_FOLDER}/{get_filename(title, artist)}.%(ext)s -x {url}")
    return os.path.isfile(f'{LIBRARY_FOLDER}/{get_filename(title, artist)}.wav')


def get_hash(title, artist) -> str:
    f_name = f'{LIBRARY_FOLDER}/{get_filename(title, artist)}.wav'
    if not os.path.isfile(f_name):
        return ''
    hash_md5 = hashlib.md5()
    with open(f_name, "rb") as f:
        for chunk in iter(lambda: f.read(4096), b""):
            hash_md5.update(chunk)
    return hash_md5.hexdigest()


def main():
    playlist = []  # type: List[Tuple[str, str, str, str]]
    with open(PLAYLIST_FILE, newline='') as fh:
        reader = csv.reader(fh, delimiter=',')
        for row in reader:
            if len(row) != 4:
                raise Exception(f'Invalid row at {row}')
            else:
                playlist.append(tuple(row))

    library = []  # type: List[Tuple[str, str, str, str]]
    with open(LIBRARY_FILE, newline='') as fh:
        reader = csv.reader(fh, delimiter=',')
        for row in reader:
            if len(row) != 4:
                raise Exception(f'Invalid row at {row}')
            else:
                library.append(tuple(row))

    for row1 in playlist:
        found_hash = ''
        found_i = -1
        for i in range(len(library)):
            row2 = library[i]
            if row1[0] == row2[0] and row1[1] == row2[1]:
                found_i = i
                if row1[2] == row2[2]:
                    found_hash = get_hash(row1[0], row1[1])
        if found_hash == '':
            if found_i != -1:
                library.pop(found_i)
            print(f'Adding and downloading {row1[0]}:{row1[1]}')
            if download(row1[0], row1[1], row1[2]):
                print('Download successful\n')
                s_hash = get_hash(row1[0], row1[1])
                library.append((row1[0], row1[1], row1[2], s_hash))
            else:
                print('Download unsuccessful\n')
        else:
            if get_hash(row1[0], row1[1]) != found_hash:
                print(f'Hash mismatch! {row1[0]}:{row1[1]}')

    with open(LIBRARY_FILE, 'w', newline='') as fh:
        writer = csv.writer(fh, delimiter=',')
        for row in library:
            writer.writerow(row)


if __name__ == '__main__':
    main()
