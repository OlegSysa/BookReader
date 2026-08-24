import { useEffect, useState } from "react";

import {
    getChapter,
    getWordTranslation,
    getSentenceTranslation
} from "../../api/ApiClient";

import "./styles.css";

interface ChapterProps {
    bookId: number;
}

export default function Chapter({ bookId }: ChapterProps) {
    const [content, setContent] = useState("");
    const [page, setPage] = useState(1);
    const [chapterIndex, setChapterIndex] = useState(2);
    const [hasNextPage, setHasNextPage] = useState(true);
    const [loading, setLoading] = useState(false);

    const [popup, setPopup] = useState<{
        text: string;
        x: number;
        y: number;
    } | null>(null);

    const loadPage = async (pageNumber: number) => {
        try {
            setLoading(true);
            if (!hasNextPage) {
                setChapterIndex(chapterIndex + 1);
            }
            const result = await getChapter(
                bookId,
                chapterIndex,
                pageNumber
            );

            setContent(result.data.content);
            setHasNextPage(!result.data.isLastChapter);

        }
        finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadPage(page);
    }, [page]);

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

    if (!content) {
        return <div>Loading...</div>;
    }

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
                        onClick={() => setPage((p) => p - 1)}
                        aria-label="Previous page"
                    >
                        ←
                    </button>

                    <span>
                        Page {page}
                    </span>

                    <button
                        disabled={!hasNextPage || loading}
                        onClick={() => setPage((p) => p + 1)}
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
                >
                    {popup.text}
                </div>
            )}
        </div>
    );
}