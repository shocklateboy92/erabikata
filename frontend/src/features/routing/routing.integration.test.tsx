import { render, screen } from '@testing-library/react';
import { App } from 'App';
import appHistory from 'appHistory';

test('redirects home page to now playing', async () => {
    appHistory.replace('/');
    render(<App />);

    const element = await screen.findByText('プレイヤーをセレクトしてね！');
    expect(element).toBeTruthy();
});

test('redirects ui leaf node to now playing', async () => {
    appHistory.replace('/ui');
    render(<App />);

    const element = await screen.findByText('プレイヤーをセレクトしてね！');
    expect(element).toBeTruthy();
});

describe('search page', () => {
    test('redirects Takoboto URL to word page', async () => {
        appHistory.replace({
            pathname: '/ui/word',
            search: 'word=https://takoboto.jp/?w=1376250'
        });
        render(<App />);

        // The word page has a title
        const title = await screen.findByText('整理');
        expect(title).toBeTruthy();

        // Make sure the word is selected, so the side panel shows up
        const definitionDrawer = await screen.findByText('Definition');
        expect(definitionDrawer).toBeTruthy();
    });

    test('redirects everything else to search page', async () => {
        appHistory.replace({
            pathname: '/ui/word',
            search: 'word=せいり'
        });
        render(<App />);

        const element = await screen.findByText('せいり search results');
        expect(element).toBeTruthy();
    });
});
