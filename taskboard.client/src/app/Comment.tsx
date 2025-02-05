import ApiService from '../Api/apiServce';
import { Button, Form } from 'react-bootstrap';
import "./Card.css";
import { useEffect, useState } from 'react';
import { CommentDto } from '../DTOs/TaskBoard/CommentDto';
import { EditCommentDto } from '../DTOs/EditCommentDto';
interface CommentProps
{
    comment: CommentDto;
    update: () => void;
}
function Comment({ comment, update }: CommentProps)
{
    const [isEditing, setIsEditing] = useState(false);
    const [text, setText] = useState<string>("");
    const [login, setLogin] = useState<string>("");

    const renderDate = (date: Date) =>
    {
        return new Date(date).toISOString().slice(0, 10)
    }

    const deleteComment = async () =>
    {
        const [, responseError] = await ApiService.DeleteCard(comment.id);
        if (responseError == null) {
            update();
        }
    }

    const editComment = async () =>
    {
        const dto: EditCommentDto = { text: text };
        const [, responseError] = await ApiService.EditComment(comment.id, dto);
        if (responseError == null) {
            update();
            setIsEditing(false);
        }
    }

    const startEditing = () =>
    {
        setIsEditing(true);
        setText(comment.text);
    }

    const GetUser = async () =>
    {
        const [response, responseError] = await ApiService.GetUser();
        if (responseError == null) {
            if (response?.value != null) {
                setLogin(response.value);
            }
        }
    }

    useEffect(() =>
    {
        GetUser();
    }, []);


    return (
        <div>
            {isEditing == true ?
                <div>
                    <Form>
                        <Form.Group className="mb-3">
                            <Form.Control type="text" placeholder="Name" value={text} onChange={(e) => { setText(e.target.value) }} />
                        </Form.Group>
                        <Button type="button" variant="primary" onClick={editComment}>
                            Confirm
                        </Button>
                        <Button type="button" variant="primary" onClick={() => { setIsEditing(false) }}>
                            Cancel
                        </Button>
                    </Form>
                </div>
                :
                <div>{comment.text}</div>
            }
            <div>
                <span>Posted </span>
                <span>{comment.user.login}</span>
            </div>
            <div>
                <span>Created </span>
                <span>{renderDate(comment.created)}</span>
            </div>
            {comment?.edited != undefined ?
                <div>
                    <span>Edited </span>
                    <span>{renderDate(comment.edited)}</span>
                </div>
                : null
            }
            {comment.user.login == login ?
                <div>
                    <Button onClick={deleteComment}>Delete</Button>
                    <Button onClick={startEditing}>Edit</Button>
                </div>
                : null
            }

        </div>
    );
}

export default Comment;