const API_BASE = import.meta.env.VITE_API_BASE_URL;

export const ENDPOINTS = {
    books: `${API_BASE}/books`,

    chapter: (bookId: string, selector: string) =>
        `${API_BASE}/documentnode?bookId=${bookId}&selector=${selector}`,

    translation: (value: string) =>
        `${API_BASE}/translation?value=${value}`,

    sentenceTranslation: (sentenceId: string, text: string) =>
        `${API_BASE}/translation/sentence-translation?sentenceId=${sentenceId}&value=${text}`,
    register: () =>
        `${API_BASE}/auth/register`,
    login: () =>
        `${API_BASE}/auth/login`,
    getUserBooks: () =>
        `${API_BASE}/book`,
    logout: () => `${API_BASE}/auth/logout`,
};