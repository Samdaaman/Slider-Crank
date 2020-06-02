import requests
import os
import base64
import json
import re
import csv
from typing import List, Tuple
from dotenv import load_dotenv


def slugify(string: str):
    string = re.sub('[^\w\s-]', '', string).strip()#.lower()
    # string = re.sub('[-\s]+', '-', string)
    return string


def get_url_and_name(f_title: str, f_artist: str) -> Tuple[str, str]:
    try:
        r_search = requests.get(f'https://www.youtube.com/results?search_query={(f_title + " by " + artist).replace(" ", "+")}')
        video_id = r_search.text.split('href="/watch?v=')[1].split('"')[0]
        assert 0 < len(video_id) < 20
        f_url = f'https://www.youtube.com/watch?v={video_id}'
        r_title = requests.get(f_url)
        return f_url, r_title.text[r_title.text.find('<title>') + 7: r_title.text.find('</title>')]
    except Exception as e:
        # print(f'Error whilst getting url... {e}')
        return '', ''


load_dotenv()

client_id = os.getenv('CLIENT_ID')
client_secret = os.getenv('CLIENT_SECRET')
playlist_id = os.getenv('PLAYLIST')
if client_id is None:
    raise Exception('Client id missing from env file')
if client_secret is None:
    raise Exception('Client secret missing from env file')
if playlist_id is None:
    raise Exception('Playlist missing from env file')

headers_auth = {'Authorization': f'Basic {base64.b64encode((client_id + ":" + client_secret).encode("utf-8")).decode("utf-8")}'}
r_auth = requests.post('https://accounts.spotify.com/api/token', data={'grant_type': 'client_credentials'}, headers=headers_auth)
if r_auth.status_code != 200:
    raise Exception('Invalid response when trying to authorize')
token = json.loads(r_auth.text).get('access_token')


headers_playlist = {'Authorization': f'Bearer {token}'}
r_playlist = requests.get(f'https://api.spotify.com/v1/playlists/{playlist_id}/tracks', headers=headers_playlist)
if r_playlist.status_code != 200:
    raise Exception('Invalid response when trying to get playlist')
playlist_data = json.loads(r_playlist.text)


playlist = []  # type: List[Tuple[str, str, str, str]]
PLAYLIST_FILE = 'playlist_map.csv'
with open(PLAYLIST_FILE, newline='') as fh:
    reader = csv.reader(fh, delimiter=',')
    for row in reader:
        if len(row) != 4:
            raise Exception(f'Invalid row at {row}')
        else:
            playlist.append(tuple(row))

for item in playlist_data.get('items'):
    name = slugify(item.get("track").get("name"))
    artist = slugify(item.get("track").get("album").get("artists")[0].get("name"))
    found = False
    for i in range(len(playlist)):
        song = playlist[i]
        if song[0] == name and song[1] == artist:
            if song[2] == '':
                print(f'Missing title for {name}:{artist}')
            found = True

    if not found:
        url, title = '', ''
        counter = 0
        while (url == '' or title == '') and counter < 5:
            url, title = get_url_and_name(name, artist)
            counter += 1
        playlist.append((name, artist, url, title))
        print(f'\nAdded song {name}:{artist}\n{title}\n{url}')

with open(PLAYLIST_FILE, 'w', newline='') as fh:
    writer = csv.writer(fh, delimiter=',')
    for row in playlist:
        writer.writerow(row)
