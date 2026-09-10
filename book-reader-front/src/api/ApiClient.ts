import { ENDPOINTS } from "./endpoints";
import type { BookModel } from "./models/book";
import type { Word } from "./models/word";
import type { PageContent } from "./models/page-content";
import type { ApiResponse } from "./models/http";

async function apiFetch(
    url: string,
    options: RequestInit = {}
): Promise<Response> {
    const response = await fetch(url, {
        ...options,
        credentials: "include"
    });

    if (response.status === 401) {
        window.location.href = "/";
        throw new Error("Unauthorized");
    }

    return response;
}

export async function getPageContent(
    bookId: number,
    pageNumber: number,
    useBookmark: boolean
): Promise<ApiResponse<PageContent>> {
    const response = await apiFetch(
        ENDPOINTS.getPageContent(bookId, pageNumber, useBookmark)
    );

    if (!response.ok) {
        throw new Error("Failed to load page");
    }

    return await response.json();
}

export async function getWordTranslation(
    value: string
): Promise<ApiResponse<string>> {
    const response = await apiFetch(
        ENDPOINTS.translation(value)
    );

    if (!response.ok) {
        throw new Error("Failed to load word translation");
    }

    return await response.json();
}

export async function getSentenceTranslation(
    sentenceId: string,
    value: string
): Promise<ApiResponse<string>> {
    const response = await apiFetch(
        ENDPOINTS.sentenceTranslation(sentenceId, value)
    );

    if (!response.ok) {
        throw new Error("Failed to load sentence translation");
    }

    return await response.json();
}

export async function getAllUserBooks(): Promise<ApiResponse<BookModel[]>> {
    const response = await apiFetch(
        ENDPOINTS.getUserBooks());

    if (!response.ok) {
        throw new Error("Failed to load books");
    }

    return await response.json();
}

export async function deleteBook(bookId: number): Promise<ApiResponse<boolean>> {
    const response = await apiFetch(
        ENDPOINTS.deleteBook(bookId), {
        method: "DELETE"
    });

    if (!response.ok) {
        throw new Error("Failed to delete book");
    }

    return await response.json();
}

export async function uploadBook(file: File, title: string, author: string) {
    const formData = new FormData();
    formData.append("File", file);
    formData.append("Title", title);
    formData.append("Author", author);

    const response = await apiFetch(ENDPOINTS.uploadBook(), {
        method: "POST",
        body: formData,
        credentials: "include"
    });

    if (!response.ok) {
        throw new Error("Failed to upload book");
    }

    return response.json();
}

export const addWordToLearning = async (value: string) => {
    const response = await apiFetch(ENDPOINTS.learnWord(), {
        headers: {
            "Content-Type": "application/json"
        },
        method: "POST",
        body: JSON.stringify(value),
        credentials: "include"
    });

    if (!response.ok) {
        throw new Error("Failed to add word to learning");
    }

    return response.json();
};

export async function getUserVocabulary(): Promise<ApiResponse<Word[]>> {
    const response = await apiFetch(
        ENDPOINTS.getUserVocabulary());

    if (!response.ok) {
        throw new Error("Failed to load user vocabulary");
    }

    return await response.json();
}