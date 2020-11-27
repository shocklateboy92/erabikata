from sys import argv
from glob import glob
from pathlib import Path
from plexapi.server import PlexServer
from plexapi.video import Show, Season, Episode
import json

auth_token = Path("../../anime-subs/x-plex-token.txt").read_text().strip()

input_root = argv[1] if len(argv) > 1 else "../../anime-subs"
files = glob(f"{input_root}/**/show_id.txt", recursive=True)

print(f"Processing {len(files)} shows")


server = PlexServer(baseurl="https://plex.apps.lasath.org", token=auth_token)
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
    with file_path.with_name("show-metadata.json").open("w", encoding="utf8") as f:
        json.dump(output, f, indent=2, ensure_ascii=False)


for file in files:
    do_process(file)
