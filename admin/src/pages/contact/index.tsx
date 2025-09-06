import { deleteContact, listContact } from "@/services/contact";
import { DeleteOutlined, EditOutlined, EyeOutlined, ManOutlined, MoreOutlined, PhoneOutlined, PlusOutlined, StopOutlined, WomanOutlined } from "@ant-design/icons";
import { ActionType, PageContainer, ProColumnType, ProTable } from "@ant-design/pro-components"
import { history } from "@umijs/max";
import { Button, Dropdown, Popconfirm, message } from "antd";
import { useRef, useState } from "react";
import BlockContactModal from "./components/block-modal";
import ContactForm from "./components/form";
import CallForm from "./components/call";

const ContactPage: React.FC = () => {

    const actionRef = useRef<ActionType>();
    const [contact, setContact] = useState<any>();
    const [openBlock, setOpenBlock] = useState<boolean>(false);
    const [openForm, setOpenForm] = useState<boolean>(false);
    const [openCall, setOpenCall] = useState<boolean>(false);

    const columns: ProColumnType<any>[] = [
        {
            title: 'STT',
            valueType: 'indexBorder',
            width: 50
        },
        {
            title: 'Họ và tên',
            dataIndex: 'name',
            render: (text, record) => {
                if (record.gender === true) {
                    return <><WomanOutlined className="text-pink-500" /> {text}</>
                }
                if (record.gender === false) {
                    return <><ManOutlined className="text-blue-500" /> {text}</>
                }
                return text;
            }
        },
        {
            title: 'Số điện thoại',
            dataIndex: 'phoneNumber'
        },
        {
            title: 'Email',
            dataIndex: 'email'
        },
        {
            title: 'Ngày tạo',
            dataIndex: 'createdDate',
            valueType: 'date',
            search: false,
            width: 100
        },
        {
            title: 'Phụ trách',
            dataIndex: 'userName',
            search: false
        },
        {
            title: 'Lượt gọi',
            dataIndex: 'callCount',
            search: false
        },
        {
            title: 'Ghi chú',
            dataIndex: 'note',
            search: false
        },
        {
            title: 'Tác vụ',
            valueType: 'option',
            render: (dom, entity) => [
                <Dropdown key="more" menu={{
                    items: [
                        {
                            key: 'view',
                            label: 'Chi tiết',
                            onClick: () => {
                                history.push(`/contact/center/${entity.id}`);
                            },
                            icon: <EyeOutlined />
                        },
                        {
                            key: 'edit',
                            label: 'Chỉnh sửa',
                            icon: <EditOutlined />
                        },
                        {
                            key: 'call',
                            label: 'Cuộc gọi',
                            onClick: () => {
                                setContact(entity);
                                setOpenCall(true);
                            },
                            icon: <PhoneOutlined />
                        },
                        {
                            key: 'block',
                            label: 'Chặn liên hệ',
                            onClick: () => {
                                setContact(entity);
                                setOpenBlock(true);
                            },
                            icon: <StopOutlined />
                        }
                    ]
                }}>
                    <Button size="small" type="dashed" icon={<MoreOutlined />} />
                </Dropdown>,
                <Popconfirm key="delete" title="Bạn có chắc chắn muốn xóa?" onConfirm={async () => {
                    await deleteContact(entity.id);
                    message.success('Xóa thành công!');
                    actionRef.current?.reload();
                }}>
                    <Button type="primary" danger icon={<DeleteOutlined />} size="small"></Button>
                </Popconfirm>
            ],
            width: 60
        }
    ]

    return (
        <PageContainer extra={<Button type="primary" icon={<PlusOutlined />} onClick={() => setOpenForm(true)}>Tạo mới</Button>}>
            <ProTable
                scroll={{
                    x: true
                }}
                actionRef={actionRef}
                search={{
                    layout: 'vertical'
                }}
                request={listContact}
                columns={columns}
            />
            <BlockContactModal open={openBlock} contact={contact} reload={() => {
                actionRef.current?.reload();
            }} onOpenChange={setOpenBlock} />
            <ContactForm open={openForm} onOpenChange={setOpenForm} reload={() => actionRef.current?.reload()} />
            <CallForm open={openCall} data={contact} onOpenChange={setOpenCall} reload={() => actionRef.current?.reload()} />
        </PageContainer>
    )
}

export default ContactPage;