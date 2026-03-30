import { useMutation, useQuery } from "@tanstack/react-query"
import { createRecipeByIdQueryOption, createRecipeCommentsQueryOption } from "../queryOptions/createRecipeQueryOption"
import { useContext, useState } from "react"
import { AuthUserContext } from "../context/AuthenticatedUserContextProvider"
import { createCommentMutationOption } from "../mutationOptions/createRegisterMutationOption"
import styles from '../css/Description.module.css'
import DescriptionCommentToggleBtn from "./DescriptionCommentToggleBtn"
import Comment from "./Comment"
import { useParams } from "react-router-dom"
import BackBtn from "./BackBtn"

const MobileDescription = () => {
    const {id} = useParams()
    const recipe = useQuery(createRecipeByIdQueryOption(id as string))

    const [isDescription, setIsDescription] = useState<boolean>(true)
    const comments = useQuery(createRecipeCommentsQueryOption(recipe.data?.id as string))
    const authUser = useContext(AuthUserContext)

    const [comment, setComment] = useState<string>("")
    const postComment = useMutation(createCommentMutationOption(recipe.data?.id as string, comment))

    const postFunction = () => {
        postComment.mutateAsync().then(_ => { location.reload() }) 
    }

    return (
        <section className={styles.data}>
            {recipe.data && <>
                <div>
                    <BackBtn/>
                    <DescriptionCommentToggleBtn description={isDescription} setDescription={setIsDescription}/>
                </div>
                <h2>{recipe.data.nev} by {recipe.data.feltoltoUsername}</h2>
                {isDescription ? 
                    <>
                        <p>Elkészítési idő: {recipe.data.elkeszitesiIdo} perc</p>
                        <h3>Hozzávalók:</h3>
                        <p>{recipe.data.hozzavalok}</p>
                        <h3>Leírás:</h3>
                        <p className={styles.description}>{recipe.data.leiras}</p>
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
            </>}
        </section>
    )
}

export default MobileDescription