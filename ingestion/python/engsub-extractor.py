from copy import Error
from multiprocessing import Pool
from sys import argv
from glob import glob
from pathlib import Path
import json
from typing import List, Tuple
import ffmpeg

MEDIA_ROOT = "/mnt/net/media"

input_root = argv[1] if len(argv) > 1 else "../../anime-subs"
show_paths = [
    Path(f) for f in glob(f"{input_root}/**/show-metadata.json", recursive=True)
]

jobs: List[Tuple[Path, Path]] = []
for show in show_paths:
    with show.open() as show_f:
        show_info = json.load(show_f)

        out_dir = show.parent.joinpath("english")
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

        if len(subs) > 1:
            raise NotImplementedError("File has more than one subtitle:", subs)

        job = ffmpeg.input(input).output(str(output))
        ffmpeg.run(job, quiet=True)

    except ffmpeg.Error as err:
        print(err.stderr)


with Pool(2) as pool:
    pool.starmap(do_extract, jobs)