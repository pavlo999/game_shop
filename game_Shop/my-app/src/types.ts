import { ReactElement } from 'react';

declare global {
    interface Window {
        google: any,
        [key: string]: any
    }
}