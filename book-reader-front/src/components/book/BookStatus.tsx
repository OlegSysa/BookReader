import "./BookStatus.css";
interface BookStatusProps {
    status?: number;
}

export default function BookStatus({ status }: BookStatusProps) {

    switch (status) {
        case 0:
            return <div>
                <span className="book-status-icon">+</span>
                <span>Saved To Storage</span>
            </div>;
        case 1:
            return <div className="book-status">
                <span className="book-status-spinner" />
                <span>created metadata</span>
            </div>;
        case 2:
            return <div className="book-status">
                <span className="book-status-spinner" />
                <span className="book-status-text" >processing started</span>
            </div>;
        case 3:
            return <div className="book-status">
                <span className="book-status-spinner" />
                <span className="book-status-text">parsed</span>
            </div>;
        case 4:
            return <div className="book-status">
                <span className="book-status-icon ready">✓ </span>
                <span className="book-status-text">ready</span>
            </div>;
        case 5:
            return <div className="book-status">
                <span className="book-status-icon failed">× </span>
                <span className="book-status-text">failed</span>
            </div>;
        default:
            return null;
    }
}