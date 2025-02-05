import { useParams } from 'react-router';
import { BoardDto } from "./../DTOs/TaskBoard/BoardDto"
import { useEffect, useState } from 'react';
import List from './List';
import { CardDto } from '../DTOs/TaskBoard/CardDto';
import CardOpened from './CardOpened';
import ApiService from '../Api/apiServce';
import "./Board.css";
import { useNavigate } from 'react-router';
import { Button, Form } from 'react-bootstrap';
import { CreateListDto } from '../DTOs/CreateListDto';
import { EditBoardDto } from '../DTOs/EditBoardDto';
import { Privatness } from '../DTOs/TaskBoard/Privatness';

function Board()
{
    const { id, cardId } = useParams();
    const [board, setBoard] = useState<BoardDto>();
    const [openedCard, setOpenedCard] = useState<CardDto>();
    const [error, setError] = useState<string>();
    const [openedCardShow, setOpenedCardShow] = useState(false);
    const [name, setName] = useState<string>("");
    const navigate = useNavigate();
    const [isEditing, setIsEditing] = useState(false);
    const [editName, setEditName] = useState<string>("");
    const [editDescription, setEditDescription] = useState<string>("");
    const [editPrivatness, setEditPrivatness] = useState<boolean>(false);

    const startEditing = () =>
    {
        setIsEditing(true);
        setEditName(board!.name);
        setEditDescription(board!.description);
        setEditPrivatness(board!.privatness == Privatness.Private ? false : true);
    }

    const editBoard = async () =>
    {
        const dto: EditBoardDto = { name: editName, description: editDescription };
        const [, responseError] = await ApiService.EditBoard(board!.id, dto);
        if (responseError == null) {
            setIsEditing(false);
            updateBoard();
        }
        if (editPrivatness == false) {
            ApiService.SetBoardPrivate(board!.id);
        }
        else {
            ApiService.SetBoardPublic(board!.id);
        }
    }

    const updateBoard = async () =>
    {
        if (id === undefined) {
            setError("BadRequest");
            return;
        }
        const idNumber = parseInt(id);
        if (isNaN(idNumber)) {
            setError("BadRequest");
            return;
        }

        const [response, responseError] = await ApiService.GetBoard(idNumber);
        if (responseError == null) {
            setError(undefined);
            if (response != null) {
                setBoard(response);
            }
            else {
                setBoard(undefined);
                setError("BadRequest");
            }
        }
        else {
            setError(responseError);
            return;
        }
        if (cardId !== undefined) {
            const cardIdNumber = parseInt(cardId);
            if (isNaN(cardIdNumber)) {
                setError("BadRequest");
                return;
            }
            const [response, responseError] = await ApiService.GetCard(cardIdNumber);
            if (responseError == null) {
                setError(undefined);
                if (response != null) {
                    setOpenedCard(response);
                    setOpenedCardShow(true);
                }
                else {
                    setOpenedCard(undefined);
                    setError("BadRequest");
                }
            }
            else {
                setError(responseError);
                return
            }
        }
        else {
            setOpenedCardShow(false);
        }
    }

    const createList = async () =>
    {
        const dto: CreateListDto = { name: name, boardId: board!.id };
        const [, responseError] = await ApiService.CreateList(dto);
        if (responseError == null) {
            updateBoard();
        }
    }

    useEffect(() =>
    {
        updateBoard();

        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [id, cardId]);

    const lists = board?.lists.sort((a, b) => a.order - b.order).map((l) => { return <List key={`list-${l.id}`} board={board} list={l} update={updateBoard} /> });

    return (
        <div>
            {isEditing == true ?
                <div>
                    <Form>
                        <Form.Group className="mb-3">
                            <Form.Label>Name</Form.Label>
                            <Form.Control type="text" placeholder="Name" value={editName} onChange={(e) => { setEditName(e.target.value) }} />
                        </Form.Group>
                        <Form.Group className="mb-3">
                            <Form.Label>Description</Form.Label>
                            <Form.Control type="text" placeholder="Description" value={editDescription} onChange={(e) => { setEditDescription(e.target.value) }} />
                        </Form.Group>
                        <Form.Group className="mb-3">
                            <Form.Check label="Public" defaultChecked={editPrivatness} onChange={(e) => { setEditPrivatness(e.target.checked) }} ></Form.Check>
                        </Form.Group>
                        <Button type="button" variant="primary" onClick={editBoard}>
                            Confirm
                        </Button>
                        <Button type="button" variant="primary" onClick={() => { setIsEditing(false) }}>
                            Cancel
                        </Button>
                    </Form>
                </div>
                :
                <div>
                    <div>Board</div>
                    <div>{board?.description}</div>
                    <Button onClick={startEditing}>Edit</Button>
                    <div>{error}</div>
                </div>
            }
            <div className="flex-row">
                {lists}
                <Form>
                    <Form.Group className="mb-3">
                        <Form.Label>Name</Form.Label>
                        <Form.Control type="text" placeholder="Name" value={name} onChange={(e) => { setName(e.target.value) }} />
                    </Form.Group>
                    <Button type="button" variant="primary" onClick={createList}>
                        Create list
                    </Button>
                </Form>
            </div>
            <div>
                <button onClick={updateBoard}></button>
            </div>
            {
                openedCard !== undefined ?
                    <div>
                        <CardOpened
                            card={openedCard!}
                            update={updateBoard}
                            show={openedCardShow}
                            onHide={() => navigate(`/board/${board!.id}`)}
                        />
                    </div>
                    : null
            }
        </div>
    );
}

export default Board;
