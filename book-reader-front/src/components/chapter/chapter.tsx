import { useEffect, useState } from "react";
import { getChapter, getTranslation } from "../../api/apiClient";
import "./styles.css";

export default function Chapter() {
    const [chapter, setChapter] = useState(null);
    const bookId = "17"; // Replace with the actual book ID
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
        debugger;
        if (!target.hasAttribute("data-word-id"))
            return;
        const loadTranslation = async () => {
            const result = await getTranslation(target.textContent || "");
            console.log(result.data);
        };
        loadTranslation();
    };

    if (!chapter) {
        return <div>Loading...</div>;
    }

    return (
        <div onClick={handleClick} dangerouslySetInnerHTML={{ __html: chapter.content }} />
    );
}