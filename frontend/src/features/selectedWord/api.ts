export const encodeSelectionParams = (
    episode: string,
    time: number,
    words: number[]
) => {
    const params = new URLSearchParams();
    params.set('episode', episode);
    params.set('time', time.toString());
    for (const word of words) {
        params.append('word', word.toString());
    }

    return params.toString();
};
