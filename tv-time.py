'''
Gets a txt file with a list of TV shows and returns the longest and shortest shows to watch
'''
import multiprocessing as mp
import pathlib
import sys
import subprocess
from subprocess import PIPE  # PIPE was not working before importing it separately


_SHOW_LENGTH_INDEX = 1


def get_show_length(show_name):
    '''Gets data from GetTvShowTotalLength through environment variable and
    return the sum of minutes of all the aired episodes'''

    # Runs GetTvShowTotalLength.exe
    process = subprocess.Popen(
        ["%GET_TVSHOW_TOTAL_LENGTH_BIN%", show_name], shell=True, stdout=PIPE)
    # Gets the output from process and decodes it.
    line = process.stdout.readline()[:-2].decode()

    # Checks if the data is valid and return the data accordingly
    if line.isnumeric():
        return ((show_name, int(line)))
    return ((show_name, -1))


def parse_shows_file(raw_shows_file_path):
    '''
    Get a txt file that returns a list containing the names of the shows
    '''
    shows_file_path = pathlib.Path(raw_shows_file_path)
    if not shows_file_path.exists():
        return []

    names = None
    with open(shows_file_path, "r") as show_names_text_file:
        names = [name.replace("\n", "").strip()
                 for name in show_names_text_file.readlines()]

    return names


if __name__ == '__main__':
    if len(sys.argv) != 2:
        print("[ERROR] wrong usage use: 'python tv_time.py arg_file1.txt")
        exit()

    # make a list of every show name
    show_names = parse_shows_file(sys.argv[1])
    if not show_names:
        print("[ERROR] could not find show names")
        exit()

    # loop through the shows_data and run 'GetTvShowTotalLength' in parallel
    pool = mp.Pool(mp.cpu_count())
    shows_data = list(pool.map(get_show_length, show_names))

    longest_show = shows_data[0]
    shortest_show = shows_data[0]

    for cur_show in shows_data[1:]:
        current_show_length = cur_show[_SHOW_LENGTH_INDEX]
        if current_show_length < 0:
            print("Could not get info for", cur_show[0])
            continue
        if longest_show[_SHOW_LENGTH_INDEX] < current_show_length:
            longest_show = cur_show
        if shortest_show[_SHOW_LENGTH_INDEX] > current_show_length:
            shortest_show = cur_show
    print("The shortest show:", shortest_show)
    print("The longest show:", longest_show)
