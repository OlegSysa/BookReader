import type { Word } from "../../api/models/word";
import { useEffect, useState } from "react";
import { getUserVocabulary } from "../../api/ApiClient";
import "./WordsList.css";
import { Download, Upload, Trash } from "lucide-react";

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
        <div className="words-toolbar">
            <button title="Import">
                <Upload size={18} />
            </button>

            <button title="Export">
                <Download size={18} />
            </button>
        </div>
        <div className="words-grid">
            <div className="grid-header">Word</div>
            <div className="grid-header">Translation</div>
            <div className="grid-header">Example</div>
            <div className="grid-header">Actions</div>

            {words.map((word) => (
                <div className="word-row" key={word.id}>
                    <div>{word.sourceValue}</div>
                    <div>{word.translation}</div>
                    <div>{word.example}</div>
                    <div className="actions">
                        <button >
                            <Trash size={14} />
                        </button>

                    </div>
                </div>
            ))}
        </div>
        <div className="pagination">

            <div className="page-buttons">
                <button>←</button>
                <button className="active">1</button>
                <button>2</button>
                <button>3</button>
                <button>→</button>
            </div>
            <div className="page-size">
                <label htmlFor="pageSize">Page size:</label>
                <select id="pageSize">
                    <option value="10">10</option>
                    <option value="20">20</option>
                    <option value="50">50</option>
                </select>
            </div>

        </div>
    </div>);
}


