import { ENDPOINTS } from "./endpoints";
import type { BookModel } from "./models/book";
import type { ApiResponse } from "./models/http";

export async function getChapter(
    bookId: string,
    selector: string
): Promise<ApiResponse<string>> {
    const response = await fetch(
        ENDPOINTS.chapter(bookId, selector)
    );

    if (!response.ok) {
        throw new Error("Failed to load chapter");
    }

    return await response.json();
}

export async function getWordTranslation(
    value: string
): Promise<ApiResponse<string>> {
    const response = await fetch(
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
    const response = await fetch(
        ENDPOINTS.sentenceTranslation(sentenceId, value)
    );

    if (!response.ok) {
        throw new Error("Failed to load sentence translation");
    }

    return await response.json();
}

export async function getAllUserBooks(): Promise<ApiResponse<BookModel[]>> {
    const response = await fetch(
        ENDPOINTS.getUserBooks(),
        { credentials: "include" });

    if (!response.ok) {
        throw new Error("Failed to load books");
    }

    return await response.json();
}