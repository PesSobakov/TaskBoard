import { useState } from 'react';
import { EditCardDto } from '../DTOs/EditCardDto';
import { CardDto } from '../DTOs/TaskBoard/CardDto';
import { Button, Form, Modal } from 'react-bootstrap';
import ApiService from '../Api/apiServce';
import { CreateCommentDto } from '../DTOs/CreateCommentDto';
import Comment from './Comment';

interface CardOpenedProps
{
    card: CardDto;
    update: () => void;
    show: boolean;
    onHide: () => void;
}

function CardOpened({ card, update, show, onHide }: CardOpenedProps)
{
    const [isEditing, setIsEditing] = useState(false);
    const [name, setName] = useState<string>("");
    const [description, setDescription] = useState<string>("");
    const [status, setStatus] = useState<string>("");
    const [dueDate, setDueDate] = useState<string>("");
    const [error, setError] = useState<string>();
    const [commentText, setCommentText] = useState<string>("");

    const renderDate = (date: Date) =>
    {
        return new Date(date).toISOString().slice(0, 10);
    }

    const editCard = async () =>
    {
        const dto: EditCardDto = { name: name, description: description, status: status, dueDate: renderDate(dueDate) };
        const [, responseError] = await ApiService.EditCard(card.id, dto);
        if (responseError == null) {
            setIsEditing(false);
            update();
        }
        else {
            setError(responseError);
        }
    }

    const startEditing = () =>
    {
        setIsEditing(true);
        setName(card.name);
        setDescription(card.description);
        setStatus(card.status);
        setDueDate(renderDate(card.dueDate));
    }

    const createComment = async () =>
    {
        const dto: CreateCommentDto = { text: commentText, cardId: card.id };
        const [, responseError] = await ApiService.CreateComment(dto);
        if (responseError == null) {
            update();
        }
    }

    const comments = card.comments.map((c) =>
    {
        return (
            <Comment comment={c} update={update}/>
        );
    })

    return (
        <Modal
            show={show}
            onHide={onHide}
            size="lg"
            aria-labelledby="contained-modal-title-vcenter"
            centered
        >
            {isEditing == true ?
                <div>
                    <Form>
                        <Form.Group className="mb-3">
                            <Form.Label>Name</Form.Label>
                            <Form.Control type="text" placeholder="Name" value={name} onChange={(e) => { setName(e.target.value) }} />
                        </Form.Group>
                        <Form.Group className="mb-3">
                            <Form.Label>Description</Form.Label>
                            <Form.Control type="text" placeholder="Description" value={description} onChange={(e) => { setDescription(e.target.value) }} />
                        </Form.Group>
                        <Form.Group className="mb-3">
                            <Form.Label>Status</Form.Label>
                            <Form.Control type="text" placeholder="Status" value={status} onChange={(e) => { setStatus(e.target.value) }} />
                        </Form.Group>
                        <Form.Group className="mb-3">
                            <Form.Label>Due date</Form.Label>
                            <Form.Control type="date" value={dueDate} onChange={(e) => { setDueDate(e.target.value) }} />
                        </Form.Group>
                        <Button type="button" variant="primary" onClick={editCard}>
                            Confirm
                        </Button>
                        <Button type="button" variant="primary" onClick={() => { setIsEditing(false) }}>
                            Cancel
                        </Button>
                        {error !== undefined ? <div>{error}</div> : null}
                    </Form>
                </div>
                :
                <div>
                    <Modal.Header closeButton>
                        <Modal.Title id="contained-modal-title-vcenter">
                            {card.name}
                        </Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <div>
                            <div>Status</div>
                            <div>{card.status}</div>
                        </div>
                        <div>
                            <div>Desctoption</div>
                            <div>{card.description}</div>
                        </div>
                        <div>
                            <div>Due Date</div>
                            <div>{renderDate(card.dueDate)}</div>
                        </div>
                        <Button onClick={startEditing}>Edit</Button>
                        <div>
                            <div>Comments</div>
                            <div>{comments}</div>
                            <Form>
                                <Form.Group className="mb-3">
                                    <Form.Control type="text" placeholder="Name" value={commentText} onChange={(e) => { setCommentText(e.target.value) }} />
                                </Form.Group>
                                <Button type="button" variant="primary" onClick={createComment}>
                                    Post comment
                                </Button>
                            </Form>
                        </div>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button onClick={onHide}>Close</Button>
                    </Modal.Footer>
                </div>
            }
        </Modal>
    );
}

export default CardOpened;