import { BrowserRouter, Routes, Route } from 'react-router-dom'

import Login from './pages/Login'
import Register from './pages/Register'
import Dashboard from './pages/Dashboard'
import RoutesPage from './pages/Routes'
import Buses from './pages/Buses'

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path='/' element={<Login />} />

        <Route path='/register' element={<Register />} />

        <Route path='/dashboard' element={<Dashboard />} />

        <Route path='/routes' element={<RoutesPage />} />

        <Route path='/buses' element={<Buses />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App