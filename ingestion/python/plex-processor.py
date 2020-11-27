from sys import argv
from glob import glob
from pathlib import Path
from multiprocessing import Pool

auth_token = Path('../../anime-subs/x-plex-token.txt').read_text().strip()

input_root = argv[1] if len(argv) > 1 else '../../anime-subs'
files = glob(f'{input_root}/**/show_id.txt', recursive=True)

print(f'Processing {len(files)} shows')


def do_process(file_path_str: str):
    file_path = Path(file_path_str)


with Pool(12) as pool:
    pool.map(do_process, files)
