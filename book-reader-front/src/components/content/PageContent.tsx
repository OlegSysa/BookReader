import { useEffect, useState } from "react";

import {
    getPageContent,
    getWordTranslation,
    getSentenceTranslation,
    addWordToLearning
} from "../../api/ApiClient";

import "./styles.css";

interface PageContent {
    bookId: number;
    onLoadingChange: (loading: boolean) => void;
}


export default function PageContent({ bookId, onLoadingChange }: PageContent) {
    const [content, setContent] = useState("");
    const [page, setPage] = useState(1);
    const [hasNextPage, setHasNextPage] = useState(true);
    const [loading, setLoading] = useState(false);

    const [popup, setPopup] = useState<{
        text: string;
        word?: string;
        x: number;
        y: number;
    } | null>(null);

    const loadPage = async (
        pageNumber: number,
        useBookmark: boolean
    ) => {
        try {
            setLoading(true);
            onLoadingChange(true);
            const result = await getPageContent(
                bookId,
                pageNumber,
                useBookmark
            );

            setPage(result.data.currentPageNumber);
            setHasNextPage(!result.data.isLastPage);
            setContent(result.data.content);
        }
        finally {
            setLoading(false);
            onLoadingChange(false);
        }
    };

    useEffect(() => {
        loadPage(page, true);
    }, [page]);

    const handleNext = async () => {
        debugger;
        if (hasNextPage) {
            await loadPage(page + 1, false);
            return;
        }
    };

    const handlePrevious = async () => {
        if (page > 1) {
            await loadPage(page - 1, false);
        }
    };

    const handleClick = (e: React.MouseEvent<HTMLDivElement>) => {
        const target = e.target as HTMLElement;

        const word = target.closest("[data-word-id]");

        if (word) {
            const loadWordTranslation = async () => {
                const result = await getWordTranslation(
                    word.textContent || ""
                );

                const rect = word.getBoundingClientRect();

                setPopup({
                    text: result.data,
                    word: word.textContent,
                    x: rect.left + rect.width / 2,
                    y: rect.top - 10
                });
            };

            loadWordTranslation();

            return;
        }

        const button = target.closest(".translate-button");

        if (button) {
            const loadSentenceTranslation = async () => {

                const sentenceContainer =
                    target.closest<HTMLElement>(".sentence");

                if (!sentenceContainer) {
                    return;
                }

                const sentenceId =
                    sentenceContainer.dataset.sentenceId;

                const sentence =
                    sentenceContainer
                        .querySelector(".sentence-text")
                        ?.textContent
                        ?.trim();

                if (!sentenceId || !sentence) {
                    return;
                }

                const result = await getSentenceTranslation(
                    sentenceId,
                    sentence
                );

                const rect = button.getBoundingClientRect();

                setPopup({
                    text: result.data,
                    x: rect.left + rect.width / 2,
                    y: rect.top - 10
                });
            };

            loadSentenceTranslation();
        }
    };

    const handleLearnWord = async (
        e: React.MouseEvent<HTMLButtonElement>
    ) => {
        e.stopPropagation();
        if (!popup?.word) {
            return;
        }

        try {
            await addWordToLearning(popup.word);
            setPopup(null);
        }
        catch {
        }
    };

    return (
        <div
            className="reader"
            onClick={() => setPopup(null)}
        >
            <div
                className={`reader-page ${loading ? "loading" : ""}`}
                onClick={(e) => e.stopPropagation()}
            >
                <div
                    className="reader-content"
                    onClick={handleClick}
                    dangerouslySetInnerHTML={{ __html: content }}
                />

                <div className="reader-pagination">
                    <button
                        disabled={page === 1 || loading}
                        onClick={handlePrevious}
                        aria-label="Previous page"
                    >
                        ←
                    </button>

                    <span>
                        Page {page}
                    </span>

                    <button
                        disabled={(!hasNextPage) || loading}
                        onClick={handleNext}
                        aria-label="Next page"
                    >
                        →
                    </button>
                </div>
            </div>

            {popup && (
                <div
                    className="translation-popup"
                    style={{
                        left: popup.x,
                        top: popup.y
                    }}
                    onClick={(e) => e.stopPropagation()}
                >
                    <span>{popup.text}</span>

                    {popup.word && (
                        <button
                            className="learn-word-button"
                            onClick={handleLearnWord}
                        >
                            +
                        </button>
                    )}
                </div>
            )}
        </div>
    )
}