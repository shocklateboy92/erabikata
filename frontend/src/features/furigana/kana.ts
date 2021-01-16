export const isKana = (text: unknown) =>
    typeof text === 'string' &&
    text.match(/^[\u3000-\u303f\u3040-\u309f\u30a0-\u30ff\uff00-\uff9f]*$/);
