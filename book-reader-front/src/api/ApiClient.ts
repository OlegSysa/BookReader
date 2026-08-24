import { ENDPOINTS } from "./endpoints";
import type { BookModel } from "./models/book";
import type { Chapter } from "./models/chapter";
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

export async function getChapter(
    bookId: number,
    chapterIndex: number,
    pageNumber: number
): Promise<ApiResponse<Chapter>> {
    const response = await apiFetch(
        ENDPOINTS.chapter(bookId, chapterIndex, pageNumber)
    );

    if (!response.ok) {
        throw new Error("Failed to load chapter");
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