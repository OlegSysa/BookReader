import type { Word } from "../../api/models/word";
import { useEffect, useState } from "react";
import { getUserVocabulary } from "../../api/ApiClient";
export default function WordsList() {
    const [words, setWords] = useState<Word[]>([]);
    const loadWords = async () => {
        try {
            const response = await getUserVocabulary();

            if (response.success) {
                setWords(response.data);
            }
        } catch (error) {
            console.error(error);
        }
    };

    useEffect(() => {
        loadWords();
    }, []);

    return (<div>
        {words.map((word) => (
            <div key={word.id}>
                <strong>{word.sourceValue}</strong> - {word.translation}
            </div>
        ))}
    </div>);
}


