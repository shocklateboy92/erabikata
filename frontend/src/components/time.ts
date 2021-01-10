import moment from 'moment';

export const formatTime = (timestamp: number) =>
    moment.utc(timestamp * 1000).format('H:mm:ss');
