#!/bin/env python3

import glob
from re import split
from sys import argv
import ass
from ass_tag_parser import parse_ass
import ass_tag_parser
import sys
import re
import json
from multiprocessing import Pool

from pathlib import Path
from ass_tag_parser.errors import ParseError

INCLUDE_STYLES = [
    "K1Norma",
    "K2Overl",
    "K3Thoug",
    "K4Notes",
    "K5Narra",
    "Default - Top",
    "Default - Italic",
    "flashback",
    "top",
    "main",
    "italics",
    "Anohana-alt",
]
INCLUDE_STYLE_PATTERNS = ["Default", "Dialogue", "Alternate"]
EXCLUDE_STYLES = ["Default2", "Alternate2"]
SUBS_ROOT = argv[1] if len(argv) > 1 else "../../anime-subs/english"
ALL_TAGS_REGEX = re.compile(r"{[^}]*}")
DRAW_MODE_REGEX = re.compile(r"{[^}]*\\p\d[^}]*}")
NEWLINE_REGEX = re.compile(r"\\n", flags=re.IGNORECASE)

files = glob.glob(f"{SUBS_ROOT}/**/*.ass", recursive=True)
print(f"Processing {len(files)} files")


def do_parse(file_path: str):
    file = Path(file_path)
    with file.open() as f:
        try:
            doc = ass.parse(f)
            results = []

            styles_file = file.with_name("include_styles.txt")
            include_styles = [
                s.strip() for s in styles_file.read_text().splitlines()
            ]

            for event in sorted(doc.events, key=lambda x: x.start):
                if event.TYPE != "Dialogue":
                    continue

                if event.style in include_styles:
                    text = []
                    try:
                        parsed = parse_ass(event.text)
                        text = [
                            a.text
                            for a in parsed
                            if isinstance(a, ass_tag_parser.AssText)
                        ]
                    except ParseError:
                        if not DRAW_MODE_REGEX.search(event.text):
                            text = ALL_TAGS_REGEX.sub("", event.text).splitlines()

                    # Split the inline line-breaks
                    text = [NEWLINE_REGEX.split(lines) for lines in text]
                    # Now flatten the list. Python's weird list-comprehensions would
                    # make this wayyyyy to confusing to do in one go.
                    text = [item for sublist in text for item in sublist]

                    results.append(
                        {
                            "text": text,
                            "time": event.start.total_seconds(),
                            "style": event.style,
                        }
                    )
                else:
                    results.append(
                        {
                            "text": [],
                            "time": event.start.total_seconds(),
                            "style": event.style,
                            "size": len(event.text),
                        }
                    )

            with open(file.with_suffix(".json"), "w") as out:
                json.dump(results, out, indent=2)
        except Exception as e:
            print(f"failed to parse {f}: {e}")


with Pool(12) as pool:
    pool.map(do_parse, files)
