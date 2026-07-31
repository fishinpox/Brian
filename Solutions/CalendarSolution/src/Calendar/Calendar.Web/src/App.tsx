import { useEffect, useState } from 'react';
import { Routes, Route, useLocation } from 'react-router-dom';
import { AnimatePresence, motion } from 'motion/react';
import { CalendarPage } from './routes/CalendarPage';
import { FollowPage } from './routes/FollowPage';
import { FontPicker } from './components/FontPicker';
import { ChatPanel } from './components/ChatPanel';
import { captureTokenFromUrl, getToken, redirectToLogin } from './lib/auth';
import { useNotificationHub } from './lib/useNotificationHub';

export function App() {
  const location = useLocation();
  const [ready, setReady] = useState(false);

  useEffect(() => {
    captureTokenFromUrl();
    if (!getToken()) {
      redirectToLogin();
      return;
    }
    setReady(true);
  }, []);

  useNotificationHub();

  if (!ready) return null;

  return (
    <div>
      <div className="flex justify-end gap-3 px-6 pt-4">
        <ChatPanel />
        <FontPicker />
      </div>
      <AnimatePresence mode="wait">
        <motion.div
          key={location.pathname}
          initial={{ opacity: 0, y: 8 }}
          animate={{ opacity: 1, y: 0 }}
          exit={{ opacity: 0 }}
          transition={{ duration: 0.15 }}
        >
          <Routes location={location}>
            <Route path="/" element={<CalendarPage />} />
            <Route path="/follow/:platform" element={<FollowPage />} />
          </Routes>
        </motion.div>
      </AnimatePresence>
    </div>
  );
}
