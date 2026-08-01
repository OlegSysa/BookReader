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

export async function getTranslation(value) {
    const response = await fetch(
        ENDPOINTS.translation(value)
    );

    if (!response.ok) {
        throw new Error("Failed to load chapter");
    }

    return await response.json();
}

