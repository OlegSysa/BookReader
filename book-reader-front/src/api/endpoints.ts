const API_BASE = import.meta.env.VITE_API_BASE_URL;
const NOTIFY_BASE = import.meta.env.VITE_NOTIFICATION_BASE_URL;
export const ENDPOINTS = {
    books: `${API_BASE}/books`,

    chapter: (bookId: number, chapterIndex: number, pageNumber: number) =>
        `${API_BASE}/documentnode?bookId=${bookId}&chapterIndex=${chapterIndex}&pageNumber=${pageNumber}`,

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
    googleAuth: (mode: "login" | "register") =>
        `${API_BASE}/auth/google?mode=${mode}`,
    uploadBook: () =>
        `${API_BASE}/book/`,
    connectNotifications: () =>
        `${NOTIFY_BASE}/api/notifications/stream/`,
    deleteBook: (id: number) =>
        `${API_BASE}/book?bookId=${id}`,
};