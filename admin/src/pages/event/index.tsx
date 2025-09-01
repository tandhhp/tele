import { DeleteOutlined, EditOutlined, FolderOutlined, PlusOutlined } from "@ant-design/icons";
import { ActionType, PageContainer, ProTable } from "@ant-design/pro-components"
import { Link, request } from "@umijs/max";
import { Button } from "antd";
import { useRef, useState } from "react";
import EventForm from "./components/form";
import { apiEventList } from "@/services/event";

const EventPage: React.FC = () => {

    const actionRef = useRef<ActionType>();
    const [event, setEvent] = useState<any>();
    const [openForm, setOpenForm] = useState<boolean>(false);

    return (
        <PageContainer extra={<Button type="primary" onClick={() => setOpenForm(true)} icon={<PlusOutlined />}>Thêm sự kiện</Button>}>
            <ProTable
                scroll={{
                    x: true
                }}
                actionRef={actionRef}
                search={{
                    layout: 'vertical'
                }}
                columns={[
                    {
                        title: '#',
                        valueType: 'indexBorder',
                        width: 40
                    },
                    {
                        title: 'Tên sự kiện',
                        dataIndex: 'name'
                    },
                    {
                        title: 'Ngày diễn ra',
                        dataIndex: 'startDate',
                        valueType: 'date',
                        search: false,
                    },
                    {
                        title: 'Thời gian',
                        dataIndex: 'startDate',
                        valueType: 'time',
                        search: false
                    },
                    {
                        title: 'Tác vụ',
                        valueType: 'option',
                        render: (_, entity) => [
                            <Button key="edit" icon={<EditOutlined />} size="small" onClick={() => {
                                setEvent(entity);
                                setOpenForm(true);
                            }}></Button>,
                            <Button key="de" icon={<DeleteOutlined />} size="small" type="primary" danger></Button>
                        ],
                        width: 100
                    }
                ]}
                request={apiEventList}
            />
            <EventForm open={openForm} onOpenChange={setOpenForm} reload={() => actionRef.current?.reload()} data={event} />
        </PageContainer>
    )
}

export default EventPage;