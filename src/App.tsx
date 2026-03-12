import Home from './pages/Home'
import { BrowserRouter, Route, Routes } from 'react-router-dom'
import Saved from './pages/Saved'
import Own from './pages/Own'
import Login from './pages/Login'
import MobileContextProvider from './context/MobileContextProvider'
import Signup from './pages/Signup'
import ForYou from './pages/ForYou'
import ContentUpload_ from './pages/ContentUpload_'
import AuthenticatedUserContextProvider from './context/AuthenticatedUserContextProvider'
import MobileDescription from './components/MobileDescription'

const App = () => {
	return (
		<MobileContextProvider>
		<AuthenticatedUserContextProvider>
			<BrowserRouter>
				<Routes>
					<Route path='/' index element={<Home/>}/>
					<Route path='/own' element={<Own/>}/>
					<Route path='/upload' element={<ContentUpload_/>}/>
					<Route path='/saved' element={<Saved/>}/>
					<Route path='/login' element={<Login/>}/>
					<Route path='/signup' element={<Signup/>}/>
					<Route path='/foryou/:id' element={<ForYou/>}/>
					<Route path='/description/:id' element={<MobileDescription/>}/>
				</Routes>
			</BrowserRouter>
		</AuthenticatedUserContextProvider>
		</MobileContextProvider>
	)
}

export default App