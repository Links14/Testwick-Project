// ─────────────────────────────────────────────────────────────
//  Testwick API Configuration
//  Change API_BASE to match your backend URL
// ─────────────────────────────────────────────────────────────
const API_BASE = 'http://localhost:5000';

const API = {
    topics:             () =>       `${API_BASE}/api/Topics`,
    topicById:          id =>       `${API_BASE}/api/Topics/${id}`,

    questions:          () =>       `${API_BASE}/api/Questions`,
    questionById:       id =>       `${API_BASE}/api/Questions/${id}`,
    questionsByTopic:   t  =>       `${API_BASE}/api/Questions/topic/${t}`,

    quizById:           id =>       `${API_BASE}/api/Quiz/${id}`,
    quizByName:         title =>    `${API_BASE}/api/Quiz/ByName?title=${title}`,
    quizCreate:         () =>       `${API_BASE}/api/Quiz`,
    quizByAdmin:        tk =>       `${API_BASE}/api/Quiz/admin/${tk}`,
    quizUpdateAdmin:    tk =>       `${API_BASE}/api/Quiz/admin/${tk}`,
    quizAddQuestion:    (tk, qid) =>`${API_BASE}/api/Quiz/admin/${tk}/AddQuestion/?questionId=${qid}`,
    quizRemoveQuestion: (tk, qid) =>`${API_BASE}/api/Quiz/admin/${tk}/RemoveQuestion/?questionId=${qid}`,
    quizByContrib:      tk =>       `${API_BASE}/api/Quiz/contribute/${tk}`,
    quizContribute:     tk =>       `${API_BASE}/api/Quiz/contribute/${tk}`,
    
    results:            () =>       `${API_BASE}/api/QuizResults`,
    userAvgResults:     (fn, ln) => `${API_BASE}/api/QuizResults/useravg/?first=${fn}&last=${ln}`,
    resultById:         id =>       `${API_BASE}/api/QuizResults/${id}`,
    resultsByQuiz:      id =>       `${API_BASE}/api/QuizResults/quiz/${id}`,
};

// ── Theme Handling ───────────────────────────────────────────
(function () {
    const saved = localStorage.getItem("theme");

    switch (saved) {
        case "light":
            document.documentElement.setAttribute("data-theme", "light");
            break;
        case "dark":
            document.documentElement.setAttribute("data-theme", "dark");
            break;
        default:
            // Default = DARK (your current design)
            document.documentElement.setAttribute("data-theme", "dark");
            break;
    }
})();

function toggleTheme() {
    const root = document.documentElement;
    const current = root.getAttribute("data-theme");

    if (current === "light") {
        root.setAttribute("data-theme", "dark");
        localStorage.setItem("theme", "dark");
    } else {
        root.setAttribute("data-theme", "light");
        localStorage.setItem("theme", "light");
    }
}

