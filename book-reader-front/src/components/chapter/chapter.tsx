import { useEffect, useState } from "react";
import { getChapter, getWordTranslation, getSentenceTranslation } from "../../api/apiClient";
import "./styles.css";

export default function Chapter() {
    const [chapter, setChapter] = useState(null);
    const bookId = "29"; // Replace with the actual book ID
    const selector = "1"; // Replace with the actual chapter selector
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

        if (target.hasAttribute("data-word-id")) {
            const loadWordTranslation = async () => {
                const result = await getWordTranslation(target.textContent || "");
                console.log(result.data);
            };
            loadWordTranslation();
            return;
        }
        if (target.classList.contains("translate-button")) {
            const loadSentenceTranslation = async () => {
                const sentenceContainer = target.closest(".sentence")!;

                const sentenceId = sentenceContainer.dataset.sentenceId!;
                const sentence = sentenceContainer
                    .querySelector(".sentence-text")!
                    .textContent!
                    .trim();

                var result = await getSentenceTranslation(sentenceId, sentence);

                console.log(result.data);
            };
            loadSentenceTranslation();
        }
    };

    if (!chapter) {
        return <div>Loading...</div>;
    }

    return (
        <div onClick={handleClick} dangerouslySetInnerHTML={{ __html: chapter.content }} />
    );
}