import { useContext, useState } from 'react'
import styles from '../css/Description.module.css'
import DescriptionCommentToggleBtn from './DescriptionCommentToggleBtn'
import { AuthUserContext } from '../context/AuthenticatedUserContextProvider'
import { useMutation, useQuery } from '@tanstack/react-query'
import { createRecipeCommentsQueryOption, type RecipeType } from '../queryOptions/createRecipeQueryOption'
import Comment from './Comment'
import { createCommentMutationOption } from '../mutationOptions/createRegisterMutationOption'

const Description = (recipe: RecipeType) => {
    const [isDescription, setIsDescription] = useState<boolean>(true)
    const comments = useQuery(createRecipeCommentsQueryOption(recipe.id))
    const authUser = useContext(AuthUserContext)

    const [comment, setComment] = useState<string>("")
    const postComment = useMutation(createCommentMutationOption(recipe.id, comment))

    const postFunction = () => {
        postComment.mutateAsync().then(_ => { location.reload() }) 
    }

    return (
        <section className={styles.data}>
            <DescriptionCommentToggleBtn description={isDescription} setDescription={setIsDescription}/>
            <h2>{recipe.nev} by {recipe.feltoltoUsername}</h2>
            {isDescription ? 
                <>
                    <p>Elkészítési idő: {recipe.elkeszitesiIdo} perc</p>
                    <h3>Hozzávalók:</h3>
                    <p>{recipe.hozzavalok}</p>
                    <h3>Leírás:</h3>
                    <p className={styles.description}>{recipe.leiras}</p>
                    <div className={styles.arrow}>
                        <i className="fa-solid fa-angle-down fa-bounce"></i>
                    </div>
                </>
            :
                <div className={styles.comments}>
                    {comments.data?.map(c => <Comment key={c.id} {...c}/>)}
                </div>
            }
            {
                authUser && !isDescription &&
                <div>
                    <i className="fa-solid fa-angle-down fa-bounce"></i>
                    <h2>Írjon kommentet:</h2>
                    <div className={styles.sendComment}>
                        <input type="text" name="comment" required onChange={(e) => setComment(e.target.value)}/>
                        <button onClick={postFunction}><i className="fa-solid fa-paper-plane fa-xl"></i></button>
                    </div>
                </div>
            }
        </section>
    )
}

export default Description