const API_BASE = "/api";

export const ENDPOINTS = {
    books: `${API_BASE}/books`,

    chapter: (bookId, selector) =>
        `/api/documentnode?bookId=${bookId}&selector=${selector}`,
    translation: (value) => `/api/translation?value=${value}`,
    sentenceTranslation: (sentenceId, text) => `/api/translation/sentence-translation?sentenceId=${sentenceId}&value=${text}`,
};