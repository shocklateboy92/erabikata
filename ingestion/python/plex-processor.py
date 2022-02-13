from sys import argv
from glob import glob
from pathlib import Path
from plexapi.server import PlexServer
from plexapi.video import Show, Season, Episode
import json

auth_token = Path("../../anime-subs/x-plex-token.txt").read_text().strip()

input_root = argv[1] if len(argv) > 1 else "../../anime-subs"
server_prefix = argv[2] + "_" if len(argv) > 2 else ""
files = glob(f"{input_root}/**/{server_prefix}show_id.txt", recursive=True)

print(f"Processing {len(files)} shows")

url = (
    "http://neelix.modem:32400"
    if server_prefix == "neelix_"
    else "https://plex.apps.lasath.org"
)
server = PlexServer(baseurl=url, token=auth_token)
anime = server.library.section("Anime")


def do_process(file_path_str: str):
    file_path = Path(file_path_str)
    show_id = int(file_path.read_text())
    show: Show = anime.fetchItem(show_id)
    season: Season = show.season(1)

    output = {
        "key": show.key,
        "title": show.title,
        "episodes": [
            [
                {"key": ep.key, "file": ep.media[0].parts[0].file}
                for ep in season.episodes()
            ]
        ],
    }
    with file_path.with_name(server_prefix + "show-metadata.json").open("w", encoding="utf8") as f:
        json.dump(output, f, indent=2, ensure_ascii=False)


for file in files:
    do_process(file)
