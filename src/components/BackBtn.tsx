import { useNavigate } from 'react-router-dom'

const BackBtn = () => {
    const navigate = useNavigate()

  return (
        <i className="fa-solid fa-arrow-left" onClick={() => navigate(-1) } style={{float: 'left'}}></i>  
    )
}

export default BackBtn