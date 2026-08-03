import { useEffect, useState } from "react";
import { getChapter, getWordTranslation, getSentenceTranslation } from "../../api/apiClient";
import "./styles.css";

export default function Chapter() {
    const [chapter, setChapter] = useState(null);
    const bookId = "31"; // Replace with the actual book ID
    const selector = "1"; // Replace with the actual chapter selector
    const [popup, setPopup] = useState<{
        text: string;
        x: number;
        y: number;
    } | null>(null);
    useEffect(() => {
        const loadChapter = async () => {
            const result = await getChapter(bookId, selector);
            debugger;
            setChapter(result.data);
        };
        loadChapter();
    }, []);


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
                const sentenceContainer = target.closest(".sentence")!;

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
            <div onClick={handleClick} dangerouslySetInnerHTML={{ __html: chapter.content }} />
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