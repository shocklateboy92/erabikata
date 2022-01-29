from copy import Error
from multiprocessing import Pool
from sys import argv
from glob import glob
from pathlib import Path
import json
from typing import List, Tuple
import ffmpeg
import pprint
import argparse

MEDIA_ROOT = "/mnt/net/media"
TRACKS_FILE_NAME = "include_tracks.txt"

lang_dir_map = { 'eng': 'english', 'jpn': 'input' }

parser = argparse.ArgumentParser(description="Extract subtitles from show media files")
parser.add_argument('input_root', type=str, default="../../anime-subs")
parser.add_argument('--language', '--lang', '-l', default="eng", dest='lang', choices=lang_dir_map.keys())
parser.add_argument('-i', '--info', action=argparse.BooleanOptionalAction, default=False)
args = parser.parse_args()

input_root = args.input_root
show_paths = [
    Path(f) for f in glob(f"{input_root}/**/show-metadata.json", recursive=True)
]

jobs: List[Tuple[Path, Path]] = []
for show in show_paths:
    with show.open() as show_f:
        show_info = json.load(show_f)

        out_dir = show.parent.joinpath(lang_dir_map[args.lang])
        if not out_dir.exists():
            out_dir.mkdir()

        for idx, ep in enumerate(show_info["episodes"][0]):
            jobs.append(
                (
                    ep["file"].replace("/mnt/data", MEDIA_ROOT),
                    out_dir.joinpath(f"{idx + 1:02}.ass"),
                )
            )


def do_extract(input: str, output: Path):
    try:
        if output.exists():
            return

        info = ffmpeg.probe(input)
        subs = [s for s in info["streams"] if s["codec_type"] == "subtitle"]

        if args.info:
            print(f'{input}:')
            for sub in subs:
                print('\t' + pprint.pformat({k: v for k, v in sub['tags'].items() if k in ['title', 'language']}))
            return

        if len(subs) > 1:
            subs = [s for s in subs if s["tags"]["language"] == args.lang]

        if len(subs) > 1:
            tracks_file = output.with_name(TRACKS_FILE_NAME)
            if not tracks_file.exists():
                raise NotImplementedError(
                    f"File has more than one subtitle but no `{TRACKS_FILE_NAME}` file:\n{pprint.pformat(subs)}"
                )

            # Should be able to use one tracks file for all languages because subs are
            # filtered by language before tracks file.
            include_tracks = [s.strip() for s in tracks_file.read_text().splitlines()]
            subs = [f for f in subs if f["tags"].get("title", None) in include_tracks]

        track_arg = next({"map": f"0:{f['index']}"} for f in subs)
        job = ffmpeg.input(input).output(str(output), **track_arg)
        ffmpeg.run(job, quiet=True)

    except ffmpeg.Error as err:
        print(err.stderr)


with Pool(2) as pool:
    pool.starmap(do_extract, jobs)
