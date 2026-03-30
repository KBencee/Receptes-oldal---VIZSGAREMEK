import { Link } from 'react-router-dom'
import styles from '../css/Login.module.css'
import HomeBtn from '../components/HomeBtn'
import { useState } from 'react'
import { useMutation } from '@tanstack/react-query'
import { createRegisterMutationOption } from '../mutationOptions/createRegisterMutationOption'

const Signup = () => {
    const [username, setUsername] = useState<string>("")
    const [password, setPassword] = useState<string>("")

    const reg = useMutation(createRegisterMutationOption(username, password))

  return (
    <div className={styles.myForm}>
        <fieldset className={styles.login}>
            <HomeBtn/>
            <h1>Regisztáció</h1>
            <label htmlFor="name">
                Felhasználónév<br/>
                <input type="text" id='name' required onChange={(e) => setUsername(e.target.value)}/><br/>
            </label>
            <label htmlFor="email">
                E-mail<br/>
                <input type="email" id='email'/><br/>
            </label>            
            <label htmlFor="pass">
                Jelszó<br/>
                <input type="password" id='pass' required onChange={(e) => setPassword(e.target.value)}/>
            </label>
            <input className={styles.btn} type="submit" value="Regisztráció" onClick={() => {reg.mutate()}}/>
            <p>Ha már van fiókod akkor&nbsp;<Link to="/login">Lépj be</Link></p>
        </fieldset>
    </div>
  )
}

export default Signup