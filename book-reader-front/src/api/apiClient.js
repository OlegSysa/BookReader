import { ENDPOINTS } from "./endpoints";

export async function getChapter(bookId, selector) {
    const response = await fetch(
        ENDPOINTS.chapter(bookId, selector)
    );

    if (!response.ok) {
        throw new Error("Failed to load chapter");
    }

    return await response.json();
}

export async function getWordTranslation(value) {
    const response = await fetch(
        ENDPOINTS.translation(value)
    );

    if (!response.ok) {
        throw new Error("Failed to load word translation");
    }

    return await response.json();
}

export async function getSentenceTranslation(sentenceId, value) {
    const response = await fetch(
        ENDPOINTS.sentenceTranslation(sentenceId, value)
    );

    if (!response.ok) {
        throw new Error("Failed to load sentence translation");
    }

    return await response.json();
}

