#!/bin/env python3

import glob
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

INCLUDE_STYLES = ['K1Norma',
                  'K2Overl', 'K3Thoug', 'K4Notes', 'K5Narra', 'Default - Top', 'Default - Italic', 'flashback', 'top', 'main', 'italics', 'Anohana-alt']
INCLUDE_STYLE_PATTERNS = ['Default', 'Dialogue', 'Alternate']
EXCLUDE_STYLES = ['Default2', 'Alternate2']
SUBS_ROOT = argv[1] if len(argv) > 1 else '../../anime-subs/english'
ALL_TAGS_REGEX = re.compile(r'{[^}]*}')
DRAW_MODE_REGEX = re.compile(r'{[^}]*\\p\d[^}]*}')

files = glob.glob(f'{SUBS_ROOT}/**/*.ass', recursive=True)
print(f"Processing {len(files)} files")


def do_parse(file_path: str):
    file = Path(file_path)
    with file.open() as f:
        try:
            doc = ass.parse(f)
            results = []

            include_styles = []
            styles_file = file.with_name("include_styles.txt")
            if (styles_file.exists()):
                include_styles = [s.strip()
                                  for s in styles_file.read_text().splitlines()]

            for event in sorted(doc.events, key=lambda x: x.start):
                if event.TYPE != 'Dialogue':
                    continue

                extract_style = event.style in include_styles if include_styles else (event.style not in EXCLUDE_STYLES and event.style in INCLUDE_STYLES or any(
                    style.lower() in event.style.lower() for style in INCLUDE_STYLE_PATTERNS))
                if extract_style:
                    text = ''
                    try:
                        parsed = parse_ass(event.text)
                        text = '\n'.join([a.text for a in parsed if isinstance(
                            a, ass_tag_parser.AssText)])
                    except ParseError:
                        if not DRAW_MODE_REGEX.search(event.text):
                            text = ALL_TAGS_REGEX.sub('', event.text)

                    results.append(
                        {'text': text, 'time': event.start.total_seconds(), 'style': event.style})
                else:
                    results.append(
                        {'time': event.start.total_seconds(), 'style': event.style, 'size': len(event.text)})

            with open(file.with_suffix('.json'), 'w') as out:
                json.dump(results, out, indent=2)
        except Exception as e:
            print(f'failed to parse {f}: {e}')


with Pool(12) as pool:
    pool.map(do_parse, files)
