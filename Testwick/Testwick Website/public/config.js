// ─────────────────────────────────────────────────────────────
//  Testwick API Configuration
//  Change API_BASE to match your backend URL
// ─────────────────────────────────────────────────────────────
const API_BASE = 'http://localhost:5000';

const API = {
  topics:              () => `${API_BASE}/api/Topics`,
  topicById:        id => `${API_BASE}/api/Topics/${id}`,

  questions:           () => `${API_BASE}/api/Questions`,
  questionById:     id => `${API_BASE}/api/Questions/${id}`,
  questionsByTopic: t  => `${API_BASE}/api/Questions/topic/${t}`,

  quizById:         id => `${API_BASE}/api/Quiz/${id}`,
  quizCreate:          () => `${API_BASE}/api/Quiz`,
  quizByAdmin:      tk => `${API_BASE}/api/Quiz/admin/${tk}`,
  quizUpdateAdmin:  tk => `${API_BASE}/api/Quiz/admin/${tk}`,
  quizAddQuestion:  (tk, qid) => `${API_BASE}/api/Quiz/admin/${tk}/AddQuestion/?questionId=${qid}`,
  quizByContrib:    tk => `${API_BASE}/api/Quiz/contribute/${tk}`,
  quizContribute:   tk => `${API_BASE}/api/Quiz/contribute/${tk}`,

  results:             () => `${API_BASE}/api/QuizResults`,
  resultById:       id => `${API_BASE}/api/QuizResults/${id}`,
  resultsByQuiz:    id => `${API_BASE}/api/QuizResults/quiz/${id}`,
};