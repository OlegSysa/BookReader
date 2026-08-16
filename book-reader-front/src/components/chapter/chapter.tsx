import { useEffect, useState, useRef } from "react";
import { getChapter, getWordTranslation, getSentenceTranslation } from "../../api/Book";
import "./styles.css";

interface ChapterProps {
    bookId: number;
}

export default function Chapter({ bookId }: ChapterProps) {
    const [chapter, setChapter] = useState<string>("");
    const selector = "3";
    const pageRef = useRef<HTMLDivElement>(null);
    const containerRef = useRef<HTMLDivElement>(null);

    const [popup, setPopup] = useState<{
        text: string;
        x: number;
        y: number;
    } | null>(null);
    useEffect(() => {
        const loadChapter = async () => {
            const result = await getChapter(bookId, selector);
            setChapter(result.data);
        };
        loadChapter();
    }, []);

    useEffect(() => {
        if (!containerRef.current || !pageRef.current) {
            return;
        }

        const pageHeight = pageRef.current.clientHeight;
        const sentences = Array.from(
            containerRef.current.querySelectorAll<HTMLElement>("[data-sentence-id]")
        );

        const pages: number[][] = [];
        let currentPage: number[] = [];

        let pageStart = 0;

        for (const sentence of sentences) {
            const top = sentence.offsetTop - pageStart;
            const bottom = top + sentence.offsetHeight;

            if (bottom > pageHeight && currentPage.length > 0) {
                pages.push(currentPage);

                currentPage = [];
                pageStart = sentence.offsetTop;
            }

            currentPage.push(Number(sentence.dataset.sentenceId));
        }

        if (currentPage.length > 0) {
            pages.push(currentPage);
        }

        console.log(pages);
    }, [chapter]);
    const handleClick = (e: React.MouseEvent<HTMLDivElement>) => {
        const target = e.target as HTMLElement;
        const word = target.closest("[data-word-id]");
        if (word) {
            const loadWordTranslation = async () => {
                const result = await getWordTranslation(word.textContent || "");
                const rect = target.getBoundingClientRect();
                setPopup({
                    text: result.data,
                    x: rect.left + rect.width / 2,
                    y: rect.top - 10
                });
                console.log(result.data);
            };
            loadWordTranslation();
            return;
        }
        const button = target.closest(".translate-button");
        if (button) {
            const loadSentenceTranslation = async () => {
                const sentenceContainer = target.closest<HTMLElement>(".sentence")!;
                const sentenceId = sentenceContainer.dataset.sentenceId!;
                const sentence = sentenceContainer
                    .querySelector(".sentence-text")!
                    .textContent!
                    .trim();

                var result = await getSentenceTranslation(sentenceId, sentence);
                const rect = target.getBoundingClientRect();
                setPopup({
                    text: result.data,
                    x: rect.left + rect.width / 2,
                    y: rect.top - 10
                });
                console.log(result.data);
            };
            loadSentenceTranslation();
        }
    };

    if (!chapter) {
        return <div>Loading...</div>;
    }
    return (
        <div onClick={() => setPopup(null)}>
            <div className="reader-page" ref={pageRef}>
                <div ref={containerRef} onClick={handleClick} dangerouslySetInnerHTML={{ __html: chapter }} />
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
        </div>
    );
}